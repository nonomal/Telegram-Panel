namespace TelegramPanel.Modules;

public readonly record struct SemVer(int Major, int Minor, int Patch) : IComparable<SemVer>
{
    public static bool TryParse(string? value, out SemVer ver)
    {
        ver = default;
        value = (value ?? string.Empty).Trim();
        if (value.Length == 0)
            return false;

        // 只支持 x.y.z，忽略预发布/构建元数据
        var parts = value.Split('.', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 1 || parts.Length > 3)
            return false;

        if (!int.TryParse(parts[0], out var major))
            return false;
        var minor = 0;
        var patch = 0;
        if (parts.Length >= 2 && !int.TryParse(parts[1], out minor))
            return false;
        if (parts.Length >= 3 && !int.TryParse(parts[2], out patch))
            return false;

        ver = new SemVer(major, minor, patch);
        return true;
    }

    public int CompareTo(SemVer other)
    {
        var c = Major.CompareTo(other.Major);
        if (c != 0) return c;
        c = Minor.CompareTo(other.Minor);
        if (c != 0) return c;
        return Patch.CompareTo(other.Patch);
    }

    public override string ToString() => $"{Major}.{Minor}.{Patch}";
}

public sealed class VersionRange
{
    private readonly List<Func<SemVer, bool>> _predicates = new();

    public static bool TryParse(string? expression, out VersionRange range, out string error)
    {
        range = new VersionRange();
        error = "";

        expression = (expression ?? string.Empty).Trim();
        if (expression.Length == 0)
        {
            error = "range 为空";
            return false;
        }

        var parts = expression.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        foreach (var raw in parts)
        {
            var token = raw.Trim();
            if (token.Length == 0)
                continue;

            if (SemVer.TryParse(token, out var exact))
            {
                range._predicates.Add(v => v == exact);
                continue;
            }

            if (token.StartsWith(">=", StringComparison.Ordinal))
            {
                if (!SemVer.TryParse(token.Substring(2), out var min))
                {
                    error = $"无法解析版本：{token}";
                    return false;
                }

                range._predicates.Add(v => v.CompareTo(min) >= 0);
                continue;
            }

            if (token.StartsWith(">", StringComparison.Ordinal))
            {
                if (!SemVer.TryParse(token.Substring(1), out var min))
                {
                    error = $"无法解析版本：{token}";
                    return false;
                }

                range._predicates.Add(v => v.CompareTo(min) > 0);
                continue;
            }

            if (token.StartsWith("<=", StringComparison.Ordinal))
            {
                if (!SemVer.TryParse(token.Substring(2), out var max))
                {
                    error = $"无法解析版本：{token}";
                    return false;
                }

                range._predicates.Add(v => v.CompareTo(max) <= 0);
                continue;
            }

            if (token.StartsWith("<", StringComparison.Ordinal))
            {
                if (!SemVer.TryParse(token.Substring(1), out var max))
                {
                    error = $"无法解析版本：{token}";
                    return false;
                }

                range._predicates.Add(v => v.CompareTo(max) < 0);
                continue;
            }

            error = $"不支持的 range token：{token}";
            return false;
        }

        if (range._predicates.Count == 0)
        {
            error = "range 无有效条件";
            return false;
        }

        return true;
    }

    public bool Contains(SemVer version)
    {
        foreach (var p in _predicates)
        {
            if (!p(version))
                return false;
        }

        return true;
    }
}

