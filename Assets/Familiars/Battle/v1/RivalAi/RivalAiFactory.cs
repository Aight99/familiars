public static class RivalAiFactory
{
    public static IRivalAi Create()
    {
        return new RandomMoveRivalAi();
    }
}
