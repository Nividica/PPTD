shader_type canvas_item;

float rand(vec2 co){
    return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

float scale(float inp, float inpMin, float inpMax, float outMin, float outMax){
	return clamp(outMin + (((inp - inpMin) / (inpMax - inpMin)) * (outMax - outMin)), min(outMin,outMax), max(outMin, outMax));
}

vec3 bgColor(vec2 uv, float time){
  float scaleFactor = 50.0;
	float y = floor(uv.y * scaleFactor);
	float timeShiftX = (time / 100.0) + uv.x;
	vec3 tpn = vec3(
		floor(timeShiftX * scaleFactor),
		floor((timeShiftX - (1.0 / scaleFactor)) * scaleFactor),
		floor((timeShiftX + (1.0 / scaleFactor)) * scaleFactor)
	);
	vec3 clr = vec3(
		rand(vec2(tpn.x, y)),
		rand(vec2(tpn.y, y)),
		rand(vec2(tpn.z, y))
	);
	
	tpn = tpn / scaleFactor;
	
	float r = max(0.1, scale(timeShiftX, tpn.y, tpn.z, clr.y, clr.z));
	float b = max(0.2, scale(timeShiftX, tpn.x, tpn.z, clr.x, clr.z));
	
	vec3 baseC = vec3(scale(uv.y, 0.0, 1.0, 0.2, 0.3),0.07,scale(uv.x, 0.0, 1.0, 0.4, 0.6));
  
  return mix(baseC, vec3(r,0.0,b), 0.05);
}

void fragment() {
	  COLOR.rgb = bgColor(SCREEN_UV, TIME);
}