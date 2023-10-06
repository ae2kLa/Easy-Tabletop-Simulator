namespace Tabletop.Local
{
    public interface LocalIRetract
    {
        public void RecordStep(LocalMapAttachArea attachArea);

        /// <summary>
        /// 跟电脑对战五子棋时，悔棋一次=撤去两枚棋子；
        /// 跟电脑对战围棋时，悔棋一次=撤去两枚棋子+复原被打吃的棋子
        /// </summary>
        public void RetractLastStep();

        public void RetractAll();
    }
}
