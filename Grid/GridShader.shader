shader_type canvas_item;

float rand(vec2 co){
    return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

void fragment() {
	// Get the texture color
	vec4 texColor = texture(TEXTURE, UV);
	
	// Calculate horizontal rules
	float horizontalRules = clamp(9.0 * abs(sin((SCREEN_UV.y * 20.0) + (TIME / 2.0))), 0.0, 1.0);
	
	vec3 color = texColor.rgb;
	
	// Meld rules with texture color
	color = color.rgb + vec3(0.3 * (1.0 - horizontalRules));
	
	float alphaMask = 0.1 + abs(cos(10.0 * SCREEN_UV.x + (TIME / 2.0)) * sin(TIME - 3.0 * SCREEN_UV.y));
	
	// Apply final color
	COLOR = vec4(color, min(texColor.a, alphaMask));
}