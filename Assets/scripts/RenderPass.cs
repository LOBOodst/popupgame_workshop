using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RenderPass : MonoBehaviour
{
    private FullScreenPassRendererFeature glitch;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        glitch.passMaterial.SetFloat("_NoiseAmount", 100);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
