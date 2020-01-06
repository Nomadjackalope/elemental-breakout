using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum PowerUps
{
	FireGainMoreEssence,
	FireBallSpeedUp1,
	FireBallSpeedUp2,
	FireBallDamageUp1,
	FireBallDamageUp2,
	FireBlast,
	FireExplosionUp,
	FireCreateFireElement,
	FireMassiveFireball,

	EarthSpawn3Elements,
	EarthQuakingHit,
	EarthGainMoreEssence,
	EarthFallOutBarrier,
	EarthNoPaddleStun,
	EarthHeavyBall,
	EarthElementPaddleBounce,
	EarthMagneticPaddle,
	EarthQuake,

	GrowthGainMoreEssence,
	GrowthBallSizeUp1,
	GrowthPaddleWidthUp,
	GrowthBallSizeUp2,
	GrowthSideSticky,
	GrowthSpawnUnderTop,
	GrowthHighPointSpawn,
	GrowthCreateGrowthElement,
	GrowthStickyPaddle,

	PoisonGainMoreEssence,
	PoisonBallDamageUp,
	PoisonPoisonSpread,
	PoisonExtendDuration,
	PoisonStartPoisoned,
	PoisonIncreaseBaseCombo,
	PoisonXHitsPoison,
	PoisonMaxCombo,
	PoisonSpray,

	ShadowGainMoreEssence,
	ShadowDisableShadow,
	ShadowLaunch,
	ShadowXHitsShadowBall,
	ShadowWormHole,
	ShadowTrail,
	ShadowTeleporterPaddle,
	ShadowMultipleBalls,
	ShadowSlowTime,

	WaterGainMoreEssence,
	WaterExtraLife1,
	WaterBouncePredictor,
	WaterExtraLife2,
	WaterGeyserLaunch,
	WaterAdditionalPaddle,
	WaterExtraLife3,
	WaterPickElementDirection,
	WaterControlBall,

	Placeholder,
		
}	
