// Import the Security framework which contains all keychain-related functions and constants
#import <Security/Security.h>

// The keychain access group that both apps must share.
// Format is: TeamID.your.group.identifier
// Both apps must declare this exact string in their entitlements under "Keychain Sharing"
static NSString* const kAccessGroup = @"F29N8H8YG5.com.hideseek.sharedkeychain";


// ── SAVE ──────────────────────────────────────────────────────────────────────
// Saves a string value to the shared keychain under the given key.
// Parameters are plain C strings because that's what Unity's DllImport sends.
// Returns true if the item was saved successfully.
bool _SaveToKeychain(const char* key, const char* value) {

    // Convert the C string key to an NSString so we can use it in the NSDictionary query
    NSString* nsKey = [NSString stringWithUTF8String:key];

    // Convert the C string value to an NSString for the same reason
    NSString* nsValue = [NSString stringWithUTF8String:value];

    // The keychain stores raw bytes, not strings — so convert the value to NSData
    NSData* data = [nsValue dataUsingEncoding:NSUTF8StringEncoding];

    // Build the query dictionary that describes the keychain item
    NSDictionary* query = @{
        // kSecClass tells the keychain what kind of item this is.
        // kSecClassGenericPassword is the standard type for arbitrary secret data.
        (__bridge id)kSecClass:           (__bridge id)kSecClassGenericPassword,

        // kSecAttrAccount is used as the "key" / identifier for this item.
        // Think of it like a dictionary key inside the keychain.
        (__bridge id)kSecAttrAccount:     nsKey,

        // kSecAttrAccessGroup scopes this item to the shared group,
        // making it visible to any app that declares the same access group.
        (__bridge id)kSecAttrAccessGroup: kAccessGroup,

        // kSecValueData is the actual secret payload — the bytes we want to store.
        (__bridge id)kSecValueData:       data
    };

    // Delete any existing item with this key first.
    // If we skip this and the item already exists, SecItemAdd will return errSecDuplicateItem.
    SecItemDelete((__bridge CFDictionaryRef)query);

    // Add the new item to the keychain.
    // Returns errSecSuccess (0) on success.
    // We cast that to bool: errSecSuccess == 0 == false... so we flip it.
    return SecItemAdd((__bridge CFDictionaryRef)query, nil) == errSecSuccess;
}


// ── LOAD ──────────────────────────────────────────────────────────────────────
// Reads a string value from the shared keychain for the given key.
// Returns a C string (which Unity will read), or NULL if the item doesn't exist.
const char* _LoadFromKeychain(const char* key) {

    // Convert the C string key to NSString for use in the query
    NSString* nsKey = [NSString stringWithUTF8String:key];

    // Build the query dictionary describing what we want to fetch
    NSDictionary* query = @{
        // We're looking for a generic password item (same class as when we saved)
        (__bridge id)kSecClass:       (__bridge id)kSecClassGenericPassword,

        // Match only the item with this specific account/key name
        (__bridge id)kSecAttrAccount: nsKey,

        // Only look inside the shared access group, not the app's private keychain
        (__bridge id)kSecAttrAccessGroup: kAccessGroup,

        // Tell the keychain we want it to return the actual data payload
        (__bridge id)kSecReturnData:  @YES,

        // Only return one result (there should only ever be one anyway)
        (__bridge id)kSecMatchLimit:  (__bridge id)kSecMatchLimitOne
    };

    // result will be populated by SecItemCopyMatching if the item is found
    CFTypeRef result = NULL;

    // Attempt to find the item. If it fails (item not found, wrong group, etc.), return NULL.
    if (SecItemCopyMatching((__bridge CFDictionaryRef)query, &result) != errSecSuccess)
        return NULL;

    // Transfer ownership of the CFTypeRef result to ARC via __bridge_transfer,
    // so we don't have to manually CFRelease it — ARC will handle memory for us.
    NSData* data = (__bridge_transfer NSData*)result;

    // Convert the raw bytes back into a readable NSString
    NSString* str = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];

    // Convert to a C string and duplicate it into a new memory buffer.
    // strdup allocates new memory that persists after this function returns,
    // so Unity can safely read it. Unity's marshaller will free this memory.
    return strdup([str UTF8String]);
}
