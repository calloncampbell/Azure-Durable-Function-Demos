﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DurableFunctionApp_StarWars.Abstrations.Models
{
    public sealed class PlanetResidents
    {
        public string PlanetName { get; set; }

        public List<Person> Residents { get; set; }
    }
}
