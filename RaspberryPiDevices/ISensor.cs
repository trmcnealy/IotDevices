namespace RaspberryPiDevices;

public interface ISensor<T> : IDisposable
    where T : ISensor<T>
{
    //public static readonly string SensorName1 = "WaterFlow1";
    //public static readonly Guid SensorId1 = Guid.Parse("F43EE6DB-EA14-4F97-863E-200000000001");

    //public static sealed Guid Uid
    //{
    //    get;
    //}
    //public static sealed string? Name
    //{
    //    get;
    //}

    //static abstract void M();
    //static abstract T P { [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]get; [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]set; }

    //static abstract event Action E;
    //static abstract T operator +(T l, T r);
    //static abstract bool operator ==(T l, T r);
    //static abstract bool operator !=(T l, T r);

    //static abstract implicit operator T(string s);
    //static abstract explicit operator string(T t);

    //static virtual void M2() {}
    //static virtual T P2 { [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]get; [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]set; }

    //static virtual event Action E2;
    //static virtual T operator -(T l, T r) { throw new NotImplementedException(); }

    //static sealed void M3() => Console.WriteLine("Default behavior");

    //static int f = 0;

    //static sealed int P1 { [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]get; [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]set; }
    //static sealed int P23 { get => f; set => f = value; }

    //static sealed event Action E13;
    //static sealed event Action E23 { add => E13 += value; remove => E13 -= value; }

    //static sealed ISensor<T> operator *(ISensor<T> l, ISensor<T> r) => l;
}
