import type { CSSProperties } from 'vue'

type CategoryLike = {
  id?: number | null
  name?: string | null
}

const CATEGORY_COLORS = [
  { bg: '#e8f3ff', text: '#1677d2', border: '#b7dcff' },
  { bg: '#eaf8ef', text: '#239557', border: '#bce8cc' },
  { bg: '#e8fbfa', text: '#0f8f85', border: '#b5e9e5' },
  { bg: '#f0edff', text: '#6254d9', border: '#d2c9ff' },
  { bg: '#fff0f6', text: '#c94076', border: '#ffd0e3' },
  { bg: '#fff4e5', text: '#b86712', border: '#ffd9a6' },
  { bg: '#f3f8e6', text: '#5f8d1c', border: '#d5e9a4' },
  { bg: '#eef5ff', text: '#2f6fca', border: '#c8dcff' },
  { bg: '#f5f0ff', text: '#7b4cc2', border: '#ddcbff' },
  { bg: '#eaf7ff', text: '#1683ad', border: '#bee6f8' },
  { bg: '#f1f3f5', text: '#59636e', border: '#d9dee5' },
]

export function accountCategoryTagStyle(category: CategoryLike): CSSProperties {
  const color = categoryColor(category)
  return {
    color: color.text,
    borderColor: color.border,
    backgroundColor: color.bg,
    '--el-tag-text-color': color.text,
    '--el-tag-border-color': color.border,
    '--el-tag-bg-color': color.bg,
  } as CSSProperties
}

export function categoryColor(category: CategoryLike) {
  const key = `${category.id ?? ''}:${category.name ?? ''}`.trim()
  const index = stringHash(key || 'uncategorized') % CATEGORY_COLORS.length
  return CATEGORY_COLORS[index]
}

function stringHash(value: string) {
  let hash = 0
  for (let i = 0; i < value.length; i += 1) {
    hash = (hash * 31 + value.charCodeAt(i)) >>> 0
  }
  return hash
}
