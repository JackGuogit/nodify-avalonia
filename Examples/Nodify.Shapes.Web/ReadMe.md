升级到11.2.3以后
    <ManagedToNativeGenerator
      Assemblies="@(_WasmAssembliesInternal);$(_MscorlibPath)"
      RuntimeIcallTableFile="$(_WasmRuntimeICallTablePath)"
      IcallOutputPath="$(_WasmICallTablePath)"
      PInvokeModules="@(_WasmPInvokeModules)"
      PInvokeOutputPath="$(_WasmPInvokeTablePath)"
      InterpToNativeOutputPath="$(_WasmInterpToNativeTablePath)"
      CacheFilePath="$(_WasmM2NCachePath)">
      <Output TaskParameter="FileWrites" ItemName="FileWrites" />
    </ManagedToNativeGenerator>
报错：Parameter types of pinvoke callback method 'System.Int32 DragEnter(System.Void*, System.Void*, System.Int32, Avalonia.Win32.Interop.UnmanagedMethods+POINT, Avalonia.Win32.Win32Com.DropEffect*)' needs to be blittable.


下面是官方的讨论，截止到2025年2月27日，并未给出解决方案
https://github.com/AvaloniaUI/Avalonia/discussions/9264