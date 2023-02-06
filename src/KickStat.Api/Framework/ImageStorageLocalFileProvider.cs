using Microsoft.Extensions.FileProviders;

namespace KickStat.UI.SiteApi.Framework;

public class ImageStorageLocalFileProvider : PhysicalFileProvider
{
    public ImageStorageLocalFileProvider(string root) : base(root)
    {
    }

    public FileStream Create(IFileInfo file)
    {
        if (file.PhysicalPath == null)
            throw new NullReferenceException("Не указан физический путь до файла");

        return Create(file.PhysicalPath, false);
    }

    public FileStream Create(string path, bool isSubPath = true) =>
        new FileInfo(isSubPath ? Path.Combine(Root, path) : path).Open(FileMode.Create, FileAccess.Write);

    public bool Delete(IFileInfo file) => file.PhysicalPath != null && Delete(file.PhysicalPath, false);

    public bool Delete(string path, bool isSubpath = true)
    {
        var file = isSubpath
            ? new FileInfo(Path.Combine(Root, path))
            : new FileInfo(path);
        if (!file.Exists) return false;

        file.Delete();
        return true;
    }
}