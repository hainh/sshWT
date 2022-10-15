using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sshWT.Animation
{
    public interface IAnimatee
    {
        int GetCurrentValue(Control control);
        void SetValue(Control control, int startValue, int newValue);
    }
}
