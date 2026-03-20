#version 330

in vec2 fragTexCoord;
in vec4 fragColor;

uniform sampler2D texture0;

out vec4 finalColor;

void main()
{
    vec4 texel = texture(texture0, fragTexCoord);
    vec3 color = texel.rgb;

    // --- Contrast boost (slight S-curve) ---
    color = clamp((color - 0.5) * 1.15 + 0.5, 0.0, 1.0);

    // --- Saturation boost ---
    float luma = dot(color, vec3(0.299, 0.587, 0.114));
    color = mix(vec3(luma), color, 1.25);

    // --- Split toning: cool shadows, warm highlights ---
    // Shadows → blue-purple tint
    vec3 shadowTint = vec3(0.15, 0.1, 0.25);
    // Highlights → warm amber
    vec3 highlightTint = vec3(0.25, 0.18, 0.05);

    float shadowWeight = 1.0 - smoothstep(0.0, 0.5, luma);
    float highlightWeight = smoothstep(0.5, 1.0, luma);

    color = mix(color, color + shadowTint, shadowWeight * 0.2);
    color = mix(color, color + highlightTint, highlightWeight * 0.15);

    // --- Vignette (subtle darkening at edges) ---
    vec2 uv = fragTexCoord - 0.5;
    float vignette = 1.0 - dot(uv, uv) * 0.8;
    vignette = smoothstep(0.3, 1.0, vignette);
    color *= mix(0.85, 1.0, vignette);

    finalColor = vec4(clamp(color, 0.0, 1.0), texel.a) * fragColor;
}
