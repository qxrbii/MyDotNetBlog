using Microsoft.AspNetCore.Identity;

public class CustomIdentityErrorDescriber : IdentityErrorDescriber
{
    // 預設錯誤
    public override IdentityError DefaultError()
    {
        return new IdentityError { Code = nameof(DefaultError), Description = $"發生未知的錯誤。" };
    }

    // 併發錯誤 (例如兩人同時修改同一筆資料)
    public override IdentityError ConcurrencyFailure()
    {
        return new IdentityError { Code = nameof(ConcurrencyFailure), Description = "資料已被其他人修改，請重新整理後再試。" };
    }

    // 密碼相關錯誤
    public override IdentityError PasswordMismatch()
    {
        return new IdentityError { Code = nameof(PasswordMismatch), Description = "密碼錯誤。" };
    }

    public override IdentityError PasswordTooShort(int length)
    {
        return new IdentityError { Code = nameof(PasswordTooShort), Description = $"密碼長度不足，至少需要 {length} 個字元。" };
    }

    public override IdentityError PasswordRequiresNonAlphanumeric()
    {
        return new IdentityError { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "密碼必須包含至少一個非英數符號 (例如 @, #, !)。" };
    }

    public override IdentityError PasswordRequiresDigit()
    {
        return new IdentityError { Code = nameof(PasswordRequiresDigit), Description = "密碼必須包含至少一個數字 ('0'-'9')。" };
    }

    public override IdentityError PasswordRequiresLower()
    {
        return new IdentityError { Code = nameof(PasswordRequiresLower), Description = "密碼必須包含至少一個小寫字母 ('a'-'z')。" };
    }

    public override IdentityError PasswordRequiresUpper()
    {
        return new IdentityError { Code = nameof(PasswordRequiresUpper), Description = "密碼必須包含至少一個大寫字母 ('A'-'Z')。" };
    }

    // 使用者名稱與 Email 重複錯誤
    public override IdentityError DuplicateUserName(string userName)
    {
        return new IdentityError { Code = nameof(DuplicateUserName), Description = $"使用者名稱 '{userName}' 已被使用。" };
    }

    public override IdentityError DuplicateEmail(string email)
    {
        return new IdentityError { Code = nameof(DuplicateEmail), Description = $"電子郵件 '{email}' 已被註冊。" };
    }

    // 使用者名稱無效
    public override IdentityError InvalidUserName(string userName)
    {
        return new IdentityError { Code = nameof(InvalidUserName), Description = $"使用者名稱 '{userName}' 無效，只能包含字母或數字。" };
    }

    public override IdentityError InvalidEmail(string email)
    {
        return new IdentityError { Code = nameof(InvalidEmail), Description = $"電子郵件 '{email}' 格式無效。" };
    }
}