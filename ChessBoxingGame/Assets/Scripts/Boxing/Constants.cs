

public static class Constants
{
    public static class Enemy
    {
        public const float BLOCKING_TIME_MIN = 1.5f;
        public const float BLOCKING_TIME_MAX = 3f;
        public const float LIGHT_PUNCH_TELE_TIME = 0.4f;
        public const float LIGHT_PUNCH_DAMAGE = 5.0f; //for single round, use 10.0f
        public const float HEAVY_PUNCH_FST_TELE_TIME = 0.65f;
        public const float HEAVY_PUNCH_SND_TELE_TIME = .35f;
        public const float HEAVY_PUNCH_DAMAGE = 9.0f; //for single round, use 15.0f
        public const float STUN_TIME = 1.5f;
        public const float FAKE_IDLE_TIME = .6f;
        public const float BLOCKING_REDUC = 0.95f;
        public const float HEALTH_MAX = 82;
        public const float POSS_BLOCKING = 0.07f;
        public const float POSS_LIGHT_PUNCH = 0.4f;
        public const float POSS_HEAVY_PUNCH = 0.5f;
        public const float POSS_IDLE = 0.1f;
    }

    public static class Player
    {
        public const float LIGHT_PUNCH_TELE_TIME = 0.3333f;
        public const float LIGHT_PUNCH_DAMAGE = 2.0f; //for single round, use 8.0f
        public const float HEAVY_PUNCH_TELE_TIME = .7f;
        public const float HEAVY_PUNCH_DAMAGE = 5.0f; //for single round, use 16.0f
        public const float BLOCKING_REDUC = 0.85f;
        public const float DODGE_IMMMUNITY_TIME = 0.5f;
        public const float DODGE_NO_IMMMUNITY_TIME = 0.2f;
        public const float DODGE_DELAY = 2f;
        public const float HEALTH_MAX = 82;
        public const float PUNCH_WAIT = 2f;
        public const float PUNCH_WAIT_SHORT = 1f;
        public const int COMBO_MAX = 3;
    }

    public const float chessTime = 60 * 2;
    public const int MAX_ROUNDS = 4;

}