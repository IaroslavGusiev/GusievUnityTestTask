#import <UIKit/UIKit.h>

extern "C" bool _iOSUtils_IsAppInstalled(const char* urlScheme)
{
    NSString *scheme = [NSString stringWithUTF8String:urlScheme];
    NSURL *url = [NSURL URLWithString:scheme];
    if (!url) return false;
    return [[UIApplication sharedApplication] canOpenURL:url];
}
