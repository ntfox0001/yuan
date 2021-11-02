/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

// Shatter Toolkit
// Copyright 2015 Gustav Olsson
using UnityEngine;

namespace ShatterToolkit.Helpers
{
    public class ShatterOnCollision : MonoBehaviour
    {
        public float requiredVelocity = 1.0f;
        public float cooldownTime = 0.5f;
        
        protected float timeSinceInstantiated;
        
        public void Update()
        {
            timeSinceInstantiated += Time.deltaTime;
        }
        
        public void OnCollisionEnter(Collision collision)
        {
            if (timeSinceInstantiated >= cooldownTime)
            {
                if (collision.relativeVelocity.magnitude >= requiredVelocity)
                {
                    if (!collision.gameObject.CompareTag("Player") && !collision.gameObject.CompareTag("DestroyedSpike")) {
                        // Find the new contact point
                        foreach (ContactPoint contact in collision.contacts) {
                            // Make sure that we don't shatter if another object in the hierarchy was hit
                            if (contact.otherCollider == collision.collider) {
                                contact.thisCollider.SendMessage("Shatter", contact.point, SendMessageOptions.DontRequireReceiver);

                                break;
                            }
                        }
                    }
                }
            }
        }

        

    }
}
