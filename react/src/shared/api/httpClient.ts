const baseUrl = import.meta.env.VITE_API_BASE_URL as string | undefined;

function getBaseUrl() {
  if (!baseUrl) {
    throw new Error('VITE_API_BASE_URL is not configured. Update .env.development.');
  }
  return baseUrl.replace(/\/$/, '');
}

export async function httpGet<T>(path: string): Promise<T> {
  const response = await fetch(`${getBaseUrl()}${path}`, {
    headers: {
      Accept: 'application/json'
    }
  });

  if (!response.ok) {
    throw new Error(`Request failed with status ${response.status}`);
  }

  return response.json() as Promise<T>;
}

export async function httpPost<T>(path: string, body?: unknown): Promise<T> {
  const response = await fetch(`${getBaseUrl()}${path}`, {
    method: 'POST',
    headers: {
      Accept: 'application/json',
      'Content-Type': 'application/json'
    },
    body: body ? JSON.stringify(body) : undefined
  });

  if (!response.ok) {
    throw new Error(`Request failed with status ${response.status}`);
  }

  return response.json() as Promise<T>;
}
