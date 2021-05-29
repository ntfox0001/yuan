using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TextureLine : MonoBehaviour
{
    public Transform Target;
    public float TextureOffsetSpeed = 2.0f;
    public float Tile = 1.0f;
    public float Width = 1.0f;
    public Material Material;
    LineRenderer mLineRenderer;
    Vector2 mOffset = Vector2.zero;
	// Use this for initialization
	void Start () {
        if (mLineRenderer == null)
        {
            mLineRenderer = GetComponent<LineRenderer>();
        }

        Refresh();
    }
    [ContextMenu("Refresh")]
    void Refresh()
    {
        var p = Target.position - transform.position;
        Vector2 scale = new Vector2();
        scale.x = Tile;
        scale.y = 1.0f;
        mLineRenderer.material = Material;
        mLineRenderer.material.SetTextureScale("_MainTex", scale);

        mLineRenderer.startWidth = Width;
        mLineRenderer.endWidth = Width;
    }

    // Update is called once per frame
    void Update () {
        mLineRenderer.SetPosition(0, transform.position);
        mLineRenderer.SetPosition(1, Target.position);

        mOffset.x += TextureOffsetSpeed * Time.deltaTime;
        mLineRenderer.material.SetTextureOffset("_MainTex", mOffset);
    }
}
