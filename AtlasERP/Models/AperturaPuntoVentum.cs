﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace AtlasERP.Models;

public partial class AperturaPuntoVentum
{
    public int Id { get; set; }

    public int? Idpuntoventa { get; set; }

    public DateTime Fecrea { get; set; }

    public string Usercrea { get; set; }

    public DateTime? Fecierre { get; set; }

    public string Ccia { get; set; }

    public string Observacion { get; set; }
}