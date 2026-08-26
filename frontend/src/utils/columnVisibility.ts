import { ref, watch } from 'vue'

export interface ColumnVisibilityOption {
  key: string
  label: string
  defaultVisible?: boolean
}

function readStoredKeys(storageKey: string): string[] | null {
  try {
    const raw = localStorage.getItem(storageKey)
    if (!raw) return null
    const parsed = JSON.parse(raw)
    return Array.isArray(parsed) ? parsed.filter((x) => typeof x === 'string') : null
  } catch {
    return null
  }
}

export function usePersistentColumnVisibility(storageKey: string, columns: ColumnVisibilityOption[]) {
  const allKeys = columns.map((x) => x.key)
  const defaultKeys = columns.filter((x) => x.defaultVisible !== false).map((x) => x.key)

  function normalize(keys: string[] | null | undefined) {
    const normalized = (keys || []).filter((x) => allKeys.includes(x))
    return normalized.length > 0 ? Array.from(new Set(normalized)) : [...defaultKeys]
  }

  const visibleColumnKeys = ref<string[]>(normalize(readStoredKeys(storageKey)))

  watch(
    visibleColumnKeys,
    (keys) => {
      const normalized = normalize(keys)
      if (normalized.length !== keys.length || normalized.some((key, index) => key !== keys[index])) {
        visibleColumnKeys.value = normalized
        return
      }
      localStorage.setItem(storageKey, JSON.stringify(normalized))
    },
    { deep: true },
  )

  function isColumnVisible(key: string) {
    return visibleColumnKeys.value.includes(key)
  }

  function resetColumns() {
    visibleColumnKeys.value = [...defaultKeys]
  }

  function showAllColumns() {
    visibleColumnKeys.value = [...allKeys]
  }

  return {
    visibleColumnKeys,
    isColumnVisible,
    resetColumns,
    showAllColumns,
  }
}
