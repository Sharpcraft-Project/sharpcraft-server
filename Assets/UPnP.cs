using System;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using Open.Nat;
using System.Collections.Generic;

namespace UPnP {
    public class NatUtility
    {
        private NatDiscoverer discoverer;
        private NatDevice device;
        private CancellationTokenSource cancellationTokenSource;
        public IPAddress externalIP;

        private async Task<NatDevice> GetDevice()
        {
            if (device == null) {
                discoverer = new NatDiscoverer();

                cancellationTokenSource = new CancellationTokenSource(10000);

                device = await discoverer.DiscoverDeviceAsync(
                    PortMapper.Upnp, cancellationTokenSource
                );

                await GetIP();
            }

            return device;
        }

        public async Task<IPAddress> GetIP()
        {
            await GetDevice();
            
            externalIP = await device.GetExternalIPAsync();

            Console.WriteLine(
                "The external IP Address is: {0} ", 
                externalIP
            );

            return externalIP;
        }

        public async Task<bool> Map( int port, string description )
        {
            try {
                await GetDevice();
                
                await device.CreatePortMapAsync(
                    new Mapping(
                        Protocol.Tcp, 
                        port, 
                        port, 
                        description
                    )
                );

                return true;
            } catch (Exception error) {
                Console.WriteLine(
                    "PortForwarding: {0}",
                    error
                );

                return false;
            }
        }

        public async Task<bool> Unmap ( int port )
        {
            try {
                await GetDevice();
                
                await device.DeletePortMapAsync(
                    new Mapping(
                        Protocol.Tcp, 
                        port, 
                        port
                    )
                );
                
                return true;
            } catch (Exception error) {
                Console.WriteLine(
                    "PortForwarding: {0}",
                    error
                );

                return false;
            }
        }

        public async Task<IEnumerable<Mapping>> Mappings () 
        {
            await GetDevice();
            return await device.GetAllMappingsAsync();
        }
    }
}