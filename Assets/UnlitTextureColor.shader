// Unlit color shader. Very simple textured and colored shader.
// - no lighting
// - no lightmap support
// - per-material color

// Change this string to move shader to new location
Shader "Custom/Unlit Texture Add Colour" {
    Properties {
        // Adds Color field we can modify
        _Color ("Main Color", Color) = (0, 0, 0, 1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }

    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
        Pass {
            Lighting Off
            
            SetTexture [_MainTex] { 
                // Sets our color as the 'constant' variable
                constantColor [_Color]
                
                // Adds color (in constant) with texture
                combine constant + texture
            }
        }
    }
}
