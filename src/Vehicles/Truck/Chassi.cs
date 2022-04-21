using Microsoft.Xna.Framework;

namespace CyberCity {
    internal class Chassi {
        public Animation front;
        public Animation back;
        public Point size { get { return new Point(front.frameWidth / 2, front.frameHeight); } private set { } }

        public Chassi(Animation front, Animation back, Point hitBoxSize) {
            this.front = front;
            this.back = back;
        }
    }
}
