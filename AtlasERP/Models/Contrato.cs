﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace AtlasERP.Models;

public partial class Contrato
{
    public int Codcontrato { get; set; }

    public DateTime? Fecinicial { get; set; }

    public DateTime? Fecfinal { get; set; }

    public string Descripcion { get; set; }

    public string Codcli { get; set; }

    public string Codusercrea { get; set; }

    public DateTime? Feccrea { get; set; }

    public string Ccia { get; set; }

    public int? Estado { get; set; }
}