import React from "react";
import { Box, Card, Flex, Text } from "@radix-ui/themes";
import FunnelChart from "./FunnelChart";
import PainPointMatrix from "./PainPointMatrix";
import ConversionArea from "./ConversionArea";
import { funnelMock, painPointsMock, generateHeatMock, conversionMock } from "../mock/mockAnalytics";

export default function AnalyticsDashboard() {
    const heatData = React.useMemo(() => generateHeatMock(), []);

    return (
        <Box mx="auto" my="6" p="4">
            <Flex gap="4" wrap="wrap">
                <Card variant="surface" style={{ flex: 1, minWidth: 380 }}>
                    <Text size="5" mb="3">Funnel</Text>
                    <FunnelChart steps={funnelMock} />
                </Card>

                <Card variant="surface" style={{ flex: 1, minWidth: 380 }}>
                    <Text size="5" mb="3">Pain Points</Text>
                    <PainPointMatrix data={painPointsMock} />
                </Card>
            </Flex>

            <Flex gap="4" wrap="wrap" mt="4">

                <Card variant="surface" style={{ flex: 1, minWidth: 420 }}>
                    <Text size="5" mb="3">Conversion Rate (30 Tage)</Text>
                    <ConversionArea data={conversionMock} />
                </Card>
            </Flex>
        </Box>
    );
}