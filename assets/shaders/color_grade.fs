#version 330

in vec2 fragTexCoord;
in vec4 fragColor;

uniform sampler2D texture0;

out vec4 finalColor;

void main()
{
    vec4 texel = texture(texture0, fragTexCoord);
    vec3 color = texel.rgb;

    // --- Contrast boost (S-curve) ---
    color = clamp((color - 0.5) * 1.18 + 0.5, 0.0, 1.0);

    // --- Saturation boost ---
    float luma = dot(color, vec3(0.299, 0.587, 0.114));
    color = mix(vec3(luma), color, 1.4);

    // --- Split toning via lerp toward tint colors ---
    // This REPLACES color in shadows/highlights rather than adding,
    // so it works even on near-black values.
    vec3 shadowColor = vec3(0.12, 0.06, 0.18);   // deep purple
    vec3 midColor = vec3(0.55, 0.35, 0.25);       // warm brown (preserve desert feel)
    vec3 highlightColor = vec3(1.0, 0.85, 0.6);   // hot amber

    float shadowMask = 1.0 - smoothstep(0.0, 0.25, luma);
    float highlightMask = smoothstep(0.6, 1.0, luma);
    float midMask = smoothstep(0.1, 0.3, luma) * (1.0 - smoothstep(0.5, 0.8, luma));

    // Blend toward tint colors (strong enough to be visible)
    color = mix(color, shadowColor, shadowMask * 0.4);
    color = mix(color, mix(color, midColor, 0.15), midMask);
    color = mix(color, highlightColor, highlightMask * 0.3);

    // --- Vignette ---
    vec2 uv = fragTexCoord - 0.5;
    float vignette = 1.0 - dot(uv, uv) * 0.8;
    vignette = smoothstep(0.3, 1.0, vignette);
    color *= mix(0.82, 1.0, vignette);

    finalColor = vec4(clamp(color, 0.0, 1.0), texel.a) * fragColor;
}
