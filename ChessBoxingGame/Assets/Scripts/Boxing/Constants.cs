

public static class Constants
{
    public static class Enemy
    {
        public const float BLOCKING_TIME_MIN = 2.0f;
        public const float BLOCKING_TIME_MAX = 4.0f;
        public const float LIGHT_PUNCH_TELE_TIME = 0.4f;
        public const float LIGHT_PUNCH_DAMAGE = 3.0f; //for single round, use 10.0f
        public const float HEAVY_PUNCH_FST_TELE_TIME = 0.75f;
        public const float HEAVY_PUNCH_SND_TELE_TIME = .35f;
        public const float HEAVY_PUNCH_DAMAGE = 5.0f; //for single round, use 15.0f
        public const float STUN_TIME = 1.25f;
        public const float FAKE_IDLE_TIME = 0.5f;
        public const float BLOCKING_REDUC = 0.95f;
        public const float HEALTH_MAX = 82;
        public const float POSS_BLOCKING = 0.07f;
        public const float POSS_LIGHT_PUNCH = 0.003f;
        public const float POSS_HEAVY_PUNCH = 0.006f;
    }

    public static class Player
    {
        public const float LIGHT_PUNCH_TELE_TIME = 0.3f;
        public const float LIGHT_PUNCH_DAMAGE = 2.0f; //for single round, use 8.0f
        public const float HEAVY_PUNCH_TELE_TIME = .3f;
        public const float HEAVY_PUNCH_DAMAGE = 4.0f; //for single round, use 16.0f
        public const float BLOCKING_REDUC = 0.8f;
        public const float DODGE_IMMMUNITY_TIME = 0.4f;
        public const float DODGE_NO_IMMMUNITY_TIME = 0.3f;
        public const float HEALTH_MAX = 82;
    }

    public const float chessTime = 60 * 3;
}