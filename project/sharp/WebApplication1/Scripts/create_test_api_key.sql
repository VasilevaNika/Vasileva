-- Создание тестового API ключа для демонстрации
-- API Key: test-api-key-12345
-- Hash: Joj04SbKXv1KYAIgc+bNkAF2JuVsPzCxlNU+Ypnt/jw=

INSERT INTO api_keys (key_hash, name, description, is_active, expires_at)
VALUES (
    'Joj04SbKXv1KYAIgc+bNkAF2JuVsPzCxlNU+Ypnt/jw=',
    'Test API Key',
    'Тестовый API ключ для демонстрации функционала',
    true,
    NULL  -- Без срока действия
)
ON CONFLICT (key_hash) DO NOTHING;

