using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpecBlocks.Options;

namespace KR_MN_Acad.Spec
{
   /// <summary>
   /// Произвольная спецификация.
   /// Должна иметь имя и уметь создавать дефолтные настройки при необхродимости
   /// </summary>
   public interface ISpecCustom
   {
      string Name { get; }
      SpecOptions GetDefaultOptions();
   }
}
