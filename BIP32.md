BIP44 & BIP32 Support
=====================

Bitcoin wallets that support BIP44 (passphrase seeds) and BIP32 (heirarchical deterministic wallets) 
all support Extended Public keys. These are used to derive the accounts within the wallet, and then the 
addresses within those accounts. However, not all wallets are standardized. Different word lists may be 
used for BIP44, and users may only have access to a **Derived** public key rather than the **Extended** Public Key.

The heirarchy of BIP44 (and BIP32) can be thought of as folders.

m/44 - top level filing cabinet
m/44'/0/k' - subfolder k
m/44'/0/k'/0/i - file within subfolder k

Now imagine the filing cabinet has a key to access all of the folders and files in it. 
That is what the **Extended Public Key** does. In order to generate addresses within the filing cabinet you have to 
specify the full path of `m/44'/0/k'/0/i`.

Now imagine you are an employee that should only have access to subfolder 1. You are provided with a derived public key 
that gives you access to read those files. Now, when generating the address of the files in that folder, you only need part of the 
path, as the key you have fills in the first part. The path to the first file is now `m/0/0`.

## Testing

The easiest way to test BIP44 / BIP32 support is to use http://bip32jp.github.io/english/ (source also located on "Tools" folder of this repo). 

## Wallet32 / Trezor

Wallet32 and Trezor allow you to create multiple accounts from within the wallet applications. Users are 
able to access the **Derived** public key, meaning they can generate addresses from under that account but not 
other accounts. As a result, when the derived public key is pasted in to Bitsie Shop, we are no longer 
concerned with the full derivation path. Rather, we just want to generate the *n-th* address in the wallet. 

### Wallet32 Derived Addresses

**Full Derivation Path**: m/44'/0'/k'/0/i
**BIP32 Tool Derivation Path**: Custom
**Custom Path**: m/0/0

**Samples:**
Extended Key: xpub6DBqy97cJjvvpACaPi9A1DWK7XBN4SBa2G6ZaNPXXdmCBU6PqdvYiRKrDyJHFL2i8Pc9WV8TCXUdY1oFMH6eQZvmmJ4bsUXznGxwongnsqn
k=0, i=0, n=0: 12wPevQiXp49cKsvfKwXTHXQK5pxV5aJt6  
k=0, i=0, n=1: 1Eh9N6EBC8NT6GU12eGfQA9nCB4NCDZtdR

### Trezor Derived Addresses

**Full Derivation Path**: m/44'/0'/k'/0'/i 
**BIP32 Tool Derivation Path**: Custom
**Custom Path**: m/0/0

**Samples:**
Extended Key: xpub6CiDQxZptHAQsdRwrXowoukJgxS9hoYwTkyGPPz98fpHjZxfa4YHwKCR8bSN7eZDCsmN8QGHKdq8ASj3THXSx38ZW83QB9funtLwdqpStSh
k=1, i=0, n=0: 17bHNUyP3xXcy31Gv8YUirjU4HqrHcSkmJ  
k=1, i=0, n=1: 1BQJpyW83mmKJJgtZpRXMgLXwRCkt1spFR