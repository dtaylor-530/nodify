﻿using System;
using System.Linq;

namespace Nodify.Demo
{


    public sealed class OperationAttribute : Attribute
    {
        public uint MaxInput { get; set; }
        public uint MinInput { get; set; }
        public bool GenerateInputNames { get; set; }
    }
}
