﻿namespace Mantle.Configuration
{
    public class ConfigurableProperty
    {
        public bool IsRequired { get; set; }
        public string[] PrioritizedSettingNames { get; set; }
        public PropertyMetadata PropertyMetadata { get; set; }
    }
}