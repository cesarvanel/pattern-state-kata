using Shared.Domain.Validator;

namespace Shared.Domain.Vo;

public record DocumentFile
{
    private DocumentFile(string fileName, float size, string extension)
    {
        FileName = fileName;
        Size = size;
        Extension = extension;
    }

    public string FileName { get; }
    public float Size { get; }
    public string Extension { get; }

    public static DocumentFile Create(string fileName, float size, string extension)
    {
        string[] allowedExtensions = [".pdf"];
        DomainValidator<DocumentFile> validator = new();
        validator.Ensure(!string.IsNullOrWhiteSpace(fileName),
                "Le nom du fichier ne peut pas être vide.")
            .Ensure(!string.IsNullOrWhiteSpace(extension),
                "L'extension du fichier ne peut pas être vide.")
            .Ensure(size > 0, "La taille du document doit être supérieure à zéro.")
            .Ensure(extension.StartsWith('.'), "L'extension doit commencer par un point.").Ensure(
                allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase),
                $"Format de document non autorisé {extension} . Seul le fichier ${allowedExtensions[0]} est accepté."
            );

        validator.ThrowIfInvalid();
        return new DocumentFile(fileName, size, extension);
    }
}