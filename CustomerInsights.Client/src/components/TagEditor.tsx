import React from "react";
import {
    Badge,
    Button,
    Flex,
    IconButton,
    Popover,
    Text,
    TextField,
} from "@radix-ui/themes";
import { PlusIcon, XIcon } from "lucide-react";

export type TagEditorProps = {
    value: string[];
    onChange: (next: string[]) => void;
    placeholder?: string;
    /** Gesamtliste der erlaubten Tags */
    suggestions?: string[];
    ariaLabel?: string;
    badgeColor?:
        | "indigo"
        | "blue"
        | "purple"
        | "gray"
        | "green"
        | "jade"
        | "red"
        | "yellow"
        | "orange"
        | "plum"
        | "pink"
        | "crimson"
        | "cyan"
        | "teal"
        | "mint"
        | "lime"
        | "grass"
        | "brown"
        | "sky";
};

const TagEditor: React.FC<TagEditorProps> = ({
                                                 value,
                                                 onChange,
                                                 placeholder = "Nach Eintrag suchen…",
                                                 suggestions = [],
                                                 ariaLabel = "Eintrag hinzufügen",
                                                 badgeColor = "indigo",
                                             }) => {
    const [open, setOpen] = React.useState(false);
    const [draft, setDraft] = React.useState("");

    const eq = (a: string, b: string) =>
        a.localeCompare(b, undefined, { sensitivity: "accent" }) === 0;

    const remaining = React.useMemo(() => {
        const chosenLower = new Set(value.map((v) => v.toLowerCase()));
        return suggestions.filter((s) => !chosenLower.has(s.toLowerCase()));
    }, [suggestions, value]);

    // 🔍 Filter: beginnt mit dem eingegebenen Text (case-insensitive)
    const filtered = React.useMemo(() => {
        const q = draft.trim().toLowerCase();
        if (!q) return remaining;
        return remaining.filter((s) => s.toLowerCase().startsWith(q));
    }, [remaining, draft]);

    const addSuggestion = (v: string) => {
        const canonical = suggestions.find((s) => eq(s, v));
        if (!canonical) return;
        if (value.some((t) => eq(t, canonical))) return;
        onChange([...value, canonical]);
        setDraft("");
        setOpen(false);
    };

    const remove = (idx: number) => {
        const next = [...value];
        next.splice(idx, 1);
        onChange(next);
    };

    const handleKeyDown: React.KeyboardEventHandler<HTMLInputElement> = (e) => {
        if (e.key === "Enter") {
            e.preventDefault();
            if (filtered.length > 0) addSuggestion(filtered[0]);
        }
    };

    React.useEffect(() => {
        if (!open) setDraft("");
    }, [open]);

    return (
        <Flex gap="2" wrap="wrap" align="center">
            {value.map((tag, i) => (
                <Badge key={`${tag}-${i}`} variant="solid" color={badgeColor}>
                    <IconButton
                        size="1"
                        style={{ height: 13, width: 13, cursor: "pointer" }}
                        aria-label={`„${tag}“ entfernen`}
                        onClick={() => remove(i)}
                    >
                        <XIcon size={12} />
                    </IconButton>
                    <Text>{tag}</Text>
                </Badge>
            ))}

            <Popover.Root open={open} onOpenChange={setOpen}>
                <Popover.Trigger>
                    <Badge
                        size="1"
                        variant="solid"
                        color={badgeColor}
                        style={{ height: 24, cursor: "pointer" }}
                        aria-label={ariaLabel}
                        role="button"
                    >
                        <PlusIcon size={14} />
                    </Badge>
                </Popover.Trigger>

                <Popover.Content size="1" maxWidth="320px">
                    <Flex direction="column" gap="2">
                        <TextField.Root
                            value={draft}
                            onChange={(e) => setDraft(e.target.value)}
                            placeholder={placeholder}
                            onKeyDown={handleKeyDown}
                            autoFocus
                        />

                        {filtered.length > 0 ? (
                            <Flex wrap="wrap" gap="2" role="listbox" aria-label="Vorschläge">
                                {filtered.map((s) => (
                                    <Button
                                        key={s}
                                        size="1"
                                        variant="surface"
                                        onClick={() => addSuggestion(s)}
                                        role="option"
                                    >
                                        {s}
                                    </Button>
                                ))}
                            </Flex>
                        ) : (
                            <Text size="1" color="gray">
                                Keine Treffer
                            </Text>
                        )}
                    </Flex>
                </Popover.Content>
            </Popover.Root>
        </Flex>
    );
};

export default TagEditor;