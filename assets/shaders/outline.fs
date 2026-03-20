#version 330

in vec2 fragTexCoord;
in vec4 fragColor;

uniform sampler2D texture0;

out vec4 finalColor;

void main()
{
    vec4 texel = texture(texture0, fragTexCoord);

    if (texel.a < 0.1)
    {
        // Transparent pixel — check 4 neighbors for opaque edge
        vec2 texelSize = 1.0 / textureSize(texture0, 0);

        float neighborAlpha = 0.0;
        neighborAlpha += texture(texture0, fragTexCoord + vec2( texelSize.x, 0.0)).a;
        neighborAlpha += texture(texture0, fragTexCoord + vec2(-texelSize.x, 0.0)).a;
        neighborAlpha += texture(texture0, fragTexCoord + vec2(0.0,  texelSize.y)).a;
        neighborAlpha += texture(texture0, fragTexCoord + vec2(0.0, -texelSize.y)).a;

        if (neighborAlpha > 0.1)
        {
            // Outline: near-black, respects entity alpha for fade-outs
            finalColor = vec4(0.02, 0.01, 0.005, 0.85 * fragColor.a);
        }
        else
        {
            finalColor = vec4(0.0);
        }
    }
    else
    {
        // Opaque pixel — pass through with tint
        finalColor = texel * fragColor;
    }
}
