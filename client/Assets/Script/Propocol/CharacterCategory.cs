using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public enum CharacterCategory
    {
        Hero = 1,
        Monster = 2,
        Item = 4,

        MI = Monster | Item,
        HM = Monster | Hero,
    }
}
