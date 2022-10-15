using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sshWT.Animation
{
    public class Animatee
    {
        public static readonly IAnimatee LocationY = new AnimateLocationY();

        class AnimateLocationY : IAnimatee
        {
            public int GetCurrentValue(Control control)
            {
                return control.Location.Y;
            }

            public void SetValue(Control control, int startValue, int newValue)
            {
                control.Location = new Point(control.Location.X, newValue);
            }
        }
    }
}
