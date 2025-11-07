import {Flex, Text, Separator, Badge} from "@radix-ui/themes";

export interface JourneyStep {
    id: string;
    name: string;
    conversionRate: number; // z. B. 84 (%)
    dropOffRate: number; // z. B. 16 (%)
    avgTime: string; // z. B. "00:34"
}

function DropOffBadge({ rate }: { rate: number }) {
    if (rate >= 40) return <Badge color="red">{rate}%</Badge>;
    if (rate >= 20) return <Badge color="amber">{rate}%</Badge>;
    return <Badge color="green">{rate}%</Badge>;
}

export function StepNode({ step }: { step: JourneyStep }) {
    return (
        <Flex
            direction="column"
            align="start"
            style={{
                width: 220,
                minHeight: 132,
                padding: 12,
                borderRadius: 10,
                background: "var(--color-panel-solid)",
                boxShadow: "inset 0 0 0 1px var(--gray-a4)"
            }}
        >
            <Text weight="medium">{step.name}</Text>

            <Separator my="2" size="2" />

            <Flex justify="between" align="center" style={{ width: "100%" }}>
                <Text size="2" color="gray">Konversion</Text>
                <Text size="2" weight="medium">{step.conversionRate}%</Text>
            </Flex>

            <Flex justify="between" align="center" mt="1" style={{ width: "100%" }}>
                <Text size="2" color="gray">Drop-off</Text>
                <DropOffBadge rate={step.dropOffRate} />
            </Flex>
        </Flex>
    );
}