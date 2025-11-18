import { Badge, Card, Flex, Skeleton, Text } from "@radix-ui/themes";

interface Props {
    title: string;
    value?: string | number;
    trend?: number | null;
    loading?: boolean;
}

export function MetricCard({ title, value, trend, loading }: Props) {
    const isUp = (trend ?? 0) >= 0;

    return (
        <Card style={{ flex: 1, padding: "1rem" }} variant="surface" mb="4">
            <Text size="2" weight="bold">{title}</Text>

            <Flex style={{ justifyContent: "space-between", marginTop: "1rem" }}>
                {loading ? (
                    <Skeleton>
                        <Text size="5">000</Text>
                    </Skeleton>
                ) : (
                    <Text size="5">{value ?? "—"}</Text>
                )}

                {loading ? (
                    <Skeleton>
                        <Badge>+0%</Badge>
                    </Skeleton>
                ) : typeof trend === "number" ? (
                    isUp ? (
                        <Badge color="green"><Text>+{trend}%</Text></Badge>
                    ) : (
                        <Badge color="red"><Text>{trend}%</Text></Badge>
                    )
                ) : null}
            </Flex>
        </Card>
    );
}