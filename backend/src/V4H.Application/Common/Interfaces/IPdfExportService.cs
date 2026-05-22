using V4H.Domain.Entities;

namespace V4H.Application.Common.Interfaces;

public interface IPdfExportService
{
    byte[] Export(Teleconsultoria teleconsultoria);
}
