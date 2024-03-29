﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mystic;

namespace NHibernateMystic
{
	class Program
	{
		static void Main(string[] args)
		{
			var nhinit = new NHibernateInitializer();
			nhinit.Initialize();
			nhinit.CreateSchema();
			var sf = nhinit.SessionFactory;

            using (var s = sf.OpenSession())
            using (var tx = s.BeginTransaction())
            {
                var p = new Persona();
                p.Nombre = "Daniel";
                p.Apellido = "Alvarez";
                s.Save(p);

                tx.Commit();
            }

			Console.WriteLine("Work done!");
			Console.ReadLine();
			sf.Close();
			nhinit.DropSchema();
		}
	}
}
