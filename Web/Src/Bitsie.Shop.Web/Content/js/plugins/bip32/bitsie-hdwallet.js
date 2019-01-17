//----------------------------------------
// HD WALLET
//----------------------------------------
var Wallet = Observable.extend({

    MESSAGE_WALLETSAVED: 'wallet-saved',
    coin: "btc_main",
    saveKey: false,
    password: null,
    keyData: null,
    COINS: {
        btc_main: {
            name: "Bitcoin",
            network: "Mainnet",
            prefix: 0,
            private_prefix: 0 + 0x80,
            bip32_public: BITCOIN_MAINNET_PUBLIC,
            bip32_private: BITCOIN_MAINNET_PRIVATE
        }
    },

    init: function () {
        this._super();
    },


    createWallet: function (walletType) {
        var _self = this;

        var bip39 = new BIP39('en');
        //create a passphrase

        // BIP39 - take the user's passphrase to generate BIP32 keys
        //var passphrase = "frame few typical school fold blue chapter craft diesel pretty action manual";
        var passphrase = bip39.generateMnemonic((12 / 3) * 32);
        passphrase = passphrase.normalize("NFKD");

        // No encryption on seed phrase.
        var seed_pw = "";
        seed_pw = seed_pw.normalize("NFKD");
        bip32_passphrase_hash = bip39.mnemonicToSeed(passphrase, seed_pw);

        // Generate BIP32
        var hasher = new jsSHA(bip32_passphrase_hash, 'HEX');
        var I = hasher.getHMAC("Bitcoin seed", "TEXT", "SHA-512", "HEX");
        var il = Crypto.util.hexToBytes(I.slice(0, 64));
        var ir = Crypto.util.hexToBytes(I.slice(64, 128));

        var gen_bip32 = new BIP32();
        gen_bip32.eckey = new Bitcoin.ECKey(il);

        gen_bip32.eckey.pub = gen_bip32.eckey.getPubPoint();
        gen_bip32.eckey.setCompressed(true);
        gen_bip32.eckey.pubKeyHash = Bitcoin.Util.sha256ripe160(gen_bip32.eckey.pub.getEncoded(true));
        gen_bip32.has_private_key = true;
        gen_bip32.chain_code = ir;
        gen_bip32.child_index = 0;
        gen_bip32.parent_fingerprint = Bitcoin.Util.hexToBytes("00000000");
        gen_bip32.version = this.COINS[this.coin].bip32_private;
        gen_bip32.depth = 0;

        gen_bip32.build_extended_public_key();
        gen_bip32.build_extended_private_key();
        bip32_source_key = new BIP32(gen_bip32.extended_private_key_string("base58"));

        // Now that we have the EXTENDED pprivate key, we want to determine what the 
        // DERIVED PUBLIC key is for the first account
        var derivedResult = bip32_source_key.derive("m/44'/0'/0'");

        // Got it. WE WANT TO STORE THIS IN THE DATABASE, AND ONLY THIS
        var derivedPublicKey = derivedResult.extended_public_key_string("base58");

        var keyData = new Object();
        keyData.MasterPublicKey = derivedPublicKey;
        keyData.walletName = walletType;
        //encrypt passphrase with secret password
        console.log(this.password);
        var secretPassword = CryptoJS.enc.Base64.parse(this.password).toString();
        var privateKeyHash = CryptoJS.AES.encrypt(passphrase, secretPassword);

        if (this.saveKey == true) {
            console.log("TRUE");
            keyData.EncryptedPrivateKey = privateKeyHash.toString();
        } else {
            keyData.EncryptedPrivateKey = null;
        }

        keyData.passphrase = passphrase;
        this.keyData = keyData;
        return keyData;
    },

    getKey: function() {
        //get password from input
        var password = CryptoJS.enc.Base64.parse($("#decrypt-password").val()).toString();

        //retrieve private key hash from db
        Bitsie.Shop.Api.post('/api/integrations/GetPrivateKeyHash', null, function (resp) {
            var decrypt = CryptoJS.AES.decrypt(resp, password);
            return decrypt;          
        });
    },

    saveExisting: function (key, walletType, callback) {
        var param = new Object();
        param.MasterPublicKey = key
        param.EncryptedPrivateKey = null;
        param.walletName = walletType;
        Bitsie.Shop.Api.post('/api/integrations/Bip44/', param, function (resp) {
            callback(resp);
        });
    }
});





