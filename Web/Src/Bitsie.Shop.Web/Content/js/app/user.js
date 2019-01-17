//----------------------------------------
// User
//----------------------------------------
var User = Observable.extend({

    MESSAGE_FORGOTPASSWORD: 'forgot-password',
    MESSAGE_RESETPASSWORD: 'reset-password',
    MESSAGE_RETRIEVED: 'retrieved',
    MESSAGE_CREATED: 'created',
    MESSAGE_DELETED: 'deleted',
    MESSAGE_LIST: 'list',
    MESSAGE_SIGNIN: 'signin',
    MESSAGE_SIGNUP: 'signup',
    MESSAGE_UPDATEPROFILE: 'update-profile',
    MESSAGE_ACCOUNTSETUP: 'update-profile',
    MESSAGE_WALLET_CREATED: 'wallet-created',
    ADDRESSES_LIST: 'list-addresses',

    
    init: function(){
        this._super();
    },

    // Public
    
    // Delete user
    "delete" : function(id) {
        var self = this;
        Bitsie.Shop.Api.post('/Api/User/Delete/' + id, null, function(resp) {
            self.notify(self.MESSAGE_DELETED, id);
        });
    },

    "export": function (opt) {
        var self = this;
        window.location = '/Api/User/Get?export=true&' + $.param(opt);
    },

    // Retrieve user
    getOne: function (id) {
        var self = this;
        Bitsie.Shop.Api.get("/Api/User/GetOne/" + id, null, function (resp) {
            self.notify(self.MESSAGE_RETRIEVED, resp);
        });
    },

    // Retrieve user
    getSecure: function (id) {
        var self = this;
        Bitsie.Shop.Api.get("/Api/User/GetSecure/" + id, null, function (resp) {
            self.notify(self.MESSAGE_RETRIEVED, resp);
        });
    },

    // Retrieve user list
    get: function (opt) {
        var self = this;
        Bitsie.Shop.Api.get("/Api/User/Get", opt, function (resp) {
            self.notify(self.MESSAGE_LIST, resp);
        });
    },

    // Retrieve user list
    getOfflineAddresses: function (opt) {
        var self = this;
        Bitsie.Shop.Api.get("/Api/OfflineAddress/Get", opt, function (resp) {
            self.notify(self.ADDRESSES_LIST, resp);
        });
    },

    // Send forgot password request
    forgotPassword: function (email) {
        var self = this;
        Bitsie.Shop.Api.post('/Api/User/ForgotPassword', {
            email: email
        }, function (resp) {
            self.notify(self.MESSAGE_FORGOTPASSWORD, resp);
        });
    },

    // Reset password
    resetPassword: function (opt) {
        var self = this;
        Bitsie.Shop.Api.post('/Api/User/ResetPassword', opt, function (resp) {
            self.notify(self.MESSAGE_RESETPASSWORD, resp);
        });
    },

    // Log user into system
    signIn: function (email, password, rememberMe, hashcashid) {
        var self = this;
       Bitsie.Shop.Api.post('/Api/User/SignIn', {
            email: email,
            password: password,
            rememberMe: rememberMe,
            hashcashid: hashcashid,
        }, function (resp) {
            if (resp.Success) {
                Bitsie.Shop.Api.Auth.setToken(resp.Token, Date.parse(resp.Expires));
            }
            self.notify(self.MESSAGE_SIGNIN, resp);
        });
    },

    // Create new user - same thing as signUp but does not auto-login
    create: function (data) {
        var self = this;
        Bitsie.Shop.Api.post('/Api/User/Create', data, function (resp) {
            self.notify(self.MESSAGE_CREATED, resp);
        });
    },

    // Sign up new user
    signUp: function (data) {
        var self = this;
        Bitsie.Shop.Api.post('/Api/User/SignUp', data, function (resp) {
            Bitsie.Shop.Api.Auth.setToken(resp.Token, Date.parse(resp.Expires));
            self.notify(self.MESSAGE_SIGNUP, resp);
        });
    },

    // Update user data
    update: function (data) {
        var self = this;
        Bitsie.Shop.Api.post('/Api/User/Update', data, function (resp) {
            self.notify(self.MESSAGE_UPDATEPROFILE, resp);
        });
    },

    // Setup user data
    setup: function (data) {
        var self = this;
        Bitsie.Shop.Api.post('/Api/User/Setup', data, function (resp) {
            self.notify(self.MESSAGE_ACCOUNTSETUP, resp);
        });
    },

    // Create User Wallet
    createWallet: function (data) {
        var self = this;
        Bitsie.Shop.Api.post('/Api/Integrations/Bip44', data, function (resp) {
            self.notify(self.MESSAGE_WALLET_CREATED, resp);
        });
    }
});
    