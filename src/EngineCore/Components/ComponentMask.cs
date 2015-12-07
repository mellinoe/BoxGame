namespace EngineCore.Components
{
    internal struct ComponentMask
    {
        public ulong Value { get; }

        public ComponentMask(ulong value)
        {
            Value = value;
        }

        public static ComponentMask GetForID(int id) => 1UL >> id;

        public static ComponentMask None => new ComponentMask(0);

        public static ComponentMask All => new ComponentMask(ulong.MaxValue);

        public static ComponentMask operator |(ComponentMask left, ComponentMask right) => left.Value | right.Value;

        public static ComponentMask operator &(ComponentMask left, ComponentMask right) => left.Value & right.Value;

        public static ComponentMask operator ^(ComponentMask left, ComponentMask right) => left.Value ^ right.Value;

        public static implicit operator ComponentMask(ulong value) => new ComponentMask(value);

        public static implicit operator ulong (ComponentMask mask) => mask.Value;
    }
}
