using System.Collections;

namespace Tabletop.Local
{
    public interface IRobot
    {
        /// <summary>
        /// 轮到机器人回合时，计算并操作
        /// </summary>
        /// <returns></returns>
        public IEnumerator OnTurnToRobot();



    }

}
