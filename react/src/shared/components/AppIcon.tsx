import {
  BadgeDollarSign,
  Bot,
  BriefcaseBusiness,
  Building2,
  CalendarClock,
  ContactRound,
  DatabaseZap,
  FileText,
  KanbanSquare,
  LayoutDashboard,
  MapPinned,
  Menu,
  Target,
  TrendingUp,
  UsersRound
} from 'lucide-react';

const icons = {
  accounts: Building2,
  activities: CalendarClock,
  admin: DatabaseZap,
  assistant: Bot,
  contacts: ContactRound,
  dashboard: LayoutDashboard,
  documents: FileText,
  forecast: TrendingUp,
  menu: Menu,
  opportunities: BadgeDollarSign,
  pipeline: KanbanSquare,
  revenue: BadgeDollarSign,
  target: Target,
  territories: MapPinned,
  users: UsersRound,
  work: BriefcaseBusiness
};

export type AppIconName = keyof typeof icons;

export function AppIcon({
  name,
  size = 20,
  className
}: {
  name: AppIconName;
  size?: number;
  className?: string;
}) {
  const Icon = icons[name];
  return <Icon aria-hidden="true" className={className} size={size} strokeWidth={1.9} />;
}
