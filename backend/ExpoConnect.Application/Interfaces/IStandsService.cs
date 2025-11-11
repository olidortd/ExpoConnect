using ExpoConnect.Contracts.Stands;

namespace ExpoConnect.Application.Interfaces;

public interface IStandsService
{
    Task<StandResponse> CreateAsync(CreateStandRequest req, CancellationToken ct);
    Task<bool> ExistsAsync(string standId, CancellationToken ct); // standId = QrCode (או StandNumber אם זה ה-PK שבחרת)
}
