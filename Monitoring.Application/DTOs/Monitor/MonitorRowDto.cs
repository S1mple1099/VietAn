namespace Monitoring.Application.DTOs.Monitor;

public record MonitorRowDto(
    string PumpId,
    DateTime Timestamp,
    string Date,
    string Time,
    string TempA,
    string TempB,
    string TempC,
    string Vrs,
    string Vst,
    string Vtr,
    string CurrentR,
    string CurrentS,
    string CurrentT,
    string Runtime,
    string TankOut,
    string TankIn);
