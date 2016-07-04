using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.Spec.Constructions
{
    /// <summary>
    /// Запись о конструкции - колонне К-1, и т.п.
    /// </summary>
    public abstract class ConstructionElement : SpecGroup.IGroupSpecElement
    {
        protected string prefix;
        public abstract string FriendlyName { get; set; }
        public abstract GroupType Group { get; set; }
        public abstract int Index { get; set; }
        public abstract string Key { get; set; }
        public abstract string Designation { get; set; }
        public abstract string Name { get; set; }
        public abstract double Weight { get; set; }
        public string Mark { get; set; }
        public ISpecBlock SpecBlock { get; set; }
        public abstract IConstructionSize Size { get; set; }
        public List<ISpecElement> Elements { get; set; } = new List<ISpecElement>();
        public double Amount { get; set; }
        public bool IsDefaultGroupings { get; set; } = false;

        public ConstructionElement(string mark, string prefix, ISpecBlock block, List<ISpecElement> elements)
        {
            Mark = mark;
            SpecBlock = block;
            this.prefix = prefix;
            Elements = elements.OrderBy(e=>e.Group).ThenBy(e=>e.Index).ThenBy(e=>e).ToList();
            Amount = elements.Sum(e => e.Amount);
        }        

        public abstract void Calc ();
        
        public virtual int CompareTo (ISpecElement other)
        {            
            var c = other as ConstructionElement;
            if (c == null) return -1;

            var res = Size.CompareTo(c.Size);
            if (res != 0) return res;

            res = Amount.CompareTo(c.Amount);
            return res;
        }
        
        public virtual bool Equals (ISpecElement other)
        {
            if (other == null) return false;
            if (Object.ReferenceEquals(this, other)) return true;
            var c = other as ConstructionElement;
            if (c == null) return false;


            var res = prefix == c.prefix &&
                Size.Equals (c.Size) &&
                Elements.SequenceEqual(c.Elements) &&
                Amount == c.Amount;
            return res;
        }

        public override int GetHashCode ()
        {
            return Key.GetHashCode();
        }

        public abstract string GetDesc ();        

        public virtual string GetNumber (string index)
        {
            return prefix + index;
        }

        public virtual string GetParamInfo ()
        {
            return Name;
        }        

        public void SetNumber (string num, int indexFirst, int indexSecond)
        {
            Mark = num;
        }

        public virtual void SumAndSetRow (SpecGroup.SpecGroupRow row, List<ISpecElement> elems)
        {
            row.Mark = Mark;
            row.Designation = Designation;
            row.Name = Name;
            int count = elems.Count;
            row.Count = count.ToString();
            row.Weight = Weight.ToString("0.000");
            row.Description = (Weight * count).ToString("0.00");            
        }

        public Dictionary<string, List<ISpecElement>> GroupsFirst (IGrouping<GroupType, ISpecElement> indexTypeGroup)
        {
            var sizes = indexTypeGroup.GroupBy(g=>((ConstructionElement)g).Size).OrderByDescending(o=>o.Key);
            return sizes.ToDictionary(k => k.Key.Key, i => i.ToList());
        }

        public Dictionary<string, List<ISpecElement>> GroupsSecond (List<ISpecElement> value)
        {            
            var uniqElems = value.GroupBy(g=>g).OrderByDescending(o=>o.Key);
            return uniqElems.ToDictionary(k => k.Key.Key, i => i.ToList());
        }
    }        
}
