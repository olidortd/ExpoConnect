namespace ExpoConnect.Contracts.Stands;

public record CreateStandRequest(
    string QrCode,
    string StandNumber,
    string CompanyName,
    string? Description,
    string Industry,
    string? ContactEmail,
    string? ContactPhone,
    string? Website,
    string? LogoUrl,
    string? BannerUrl,
    string ExhibitorId);

public record StandResponse(
    string QrCode,
    string StandNumber,
    string CompanyName);
