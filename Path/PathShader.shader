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
	
	// Calculate a nice gradient slowly moving from left to right
	float gradeX = max(0.25, abs(sin(SCREEN_UV.x - (TIME / 10.0))));
	
	// Apply gradient only to green channel
	color.g = mix(color.g, gradeX, 0.5);
	
	// Meld rules with texture color, ignoring alpha
	 color = color.rgb + vec3(0.3 * (1.0 - horizontalRules));
	
	// Apply final color, with alpha from texture
	COLOR = vec4(color, texColor.a);
}