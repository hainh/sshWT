using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sshWT.Animation
{
    public class Animator
    {
        public static void Animate(IList<Control> controls, IAnimatee animatee, EasingFunc easingFunc, int changeBy, int duration, int delay)
        {
            if (controls.Count == 0) return;
            Task.Run(async () =>
            {
                if (delay > 0) await Task.Delay(delay);
                var now = DateTime.UtcNow;
                var currentValues = controls.Select(control => animatee.GetCurrentValue(control)).ToArray();

                while (true)
                {
                    if (delay <= 0) await Task.Delay(15);
                    delay = 0;
                    var elapsed = (DateTime.UtcNow - now).TotalMilliseconds;
                    if (elapsed > duration)
                    {
                        setValue(changeBy);
                        return;
                    }
                    var valueBy = (int)(easingFunc(elapsed / duration) * changeBy);
                    setValue(valueBy);
                }

                void setValue(int valueBy)
                {
                    controls[0].Invoke(() =>
                    {
                        for (var i = 0; i < controls.Count; i++)
                        {
                            var control = controls[i];
                            animatee.SetValue(control, currentValues[i], currentValues[i] + valueBy);
                        }
                    });
                }
            });
        }
    }
}
