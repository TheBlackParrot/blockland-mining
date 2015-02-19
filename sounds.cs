datablock AudioProfile(pop_high)
{
	filename = "./sounds/pop_high.wav";
	description = AudioClosest3d;
	preload = true;
};
datablock AudioProfile(pop_low:pop_high) { filename = "./sounds/pop_low.wav"; };
datablock AudioProfile(explosion_far:pop_high) { filename = "./sounds/explosion_far.wav"; };
datablock AudioProfile(explosion_neither:pop_high) { filename = "./sounds/explosion_neither.wav"; };
datablock AudioProfile(explosion_close:pop_high) { filename = "./sounds/explosion_close.wav"; };