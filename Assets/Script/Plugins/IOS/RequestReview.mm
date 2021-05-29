#import <StoreKit/StoreKit.h>
#import "RequestReview.h"


#if defined(__cplusplus)
extern "C"{
#endif

    void requestReview()
    {
        [SKStoreReviewController requestReview];
    }

#if defined(__cplusplus)
}
#endif
