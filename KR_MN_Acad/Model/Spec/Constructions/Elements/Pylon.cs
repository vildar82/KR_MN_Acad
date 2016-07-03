﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.Spec.Constructions.Elements
{
    public class Pylon : ConstructionElement
    {
        public override string Designation { get; set; } = "";
        public override string FriendlyName { get; set; } = "Пилон монолитный";
        public override GroupType Group { get; set; } = GroupType.MonolithPylon;
        public override int Index { get; set; } = 0;
        public override string Key { get; set; }
        public override string Name { get; set; }
        public override IConstructionSize Size { get; set; }
        public override double Weight { get; set; }

        public Pylon (int length, int width, int height, string mark, ISpecBlock block, List<ISpecElement> elements) :
            base(mark, "П-", block, elements)
        {
            Size = new ConstructionSize(length, width, height);
            Name = $"{FriendlyName}, {length}х{width}, h={height}мм";
            Key = Name + string.Join(";", elements.Select(e=>e.Key + e.Amount))+ Amount;
        }

        public override void Calc ()
        {
            Weight = Elements.Sum(e => e.Amount);
        }

        public override string GetDesc ()
        {
            return Name;
        }        
    }
}
