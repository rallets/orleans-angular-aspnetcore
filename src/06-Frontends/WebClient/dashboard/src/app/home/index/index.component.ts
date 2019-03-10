import { Component, OnInit } from '@angular/core';
import { NgbCarouselConfig } from '@ng-bootstrap/ng-bootstrap';

class SlideInfo {
  url: string;
  title: string;
  description: string;
  extUrl: string;
}

@Component({
  selector: 'app-orders-index',
  templateUrl: './index.component.html',
  styleUrls: ['./index.component.scss']
})
export class IndexComponent implements OnInit {

  images: SlideInfo[] = [
    {
      url: 'assets/images/orleans/actor_middle_tier.png',
      title: 'Orleans as a Stateful Middle Tier',
      description: '',
      extUrl: 'https://dotnet.github.io/orleans/Documentation/index.html'
    },

    {
      url: 'assets/images/orleans/frontend_cluster.png',
      title: 'Frontend cluster',
      description: '',
      extUrl: 'https://dotnet.github.io/orleans/Documentation/clusters_and_clients/'
    },

    {
      url: 'assets/images/orleans/grain_lifecycle.png',
      title: 'Grain lifecycle',
      description: '',
      extUrl: 'https://dotnet.github.io/orleans/Documentation/index.html'
    },

    {
      url: 'assets/images/orleans/heterogeneous silos.png',
      title: 'Hetererogeneus silos',
      description: '',
      extUrl: 'https://dotnet.github.io/orleans/Documentation/clusters_and_clients/heterogeneous_silos.html'
    },

    {
      url: 'assets/images/orleans/n-tier.png',
      title: 'Common N-tier',
      description: 'Most of today’s high scale properties are built as a composition of stateless n-tier services with most of the application logic residing in the middle tier.',
      extUrl: 'https://dotnet.github.io/orleans/Documentation/index.html'
    },

    {
      url: 'assets/images/orleans/orleans dashboard.png',
      title: 'Orleans dashboard',
      description: 'An admin dashboard for Microsoft Orleans',
      extUrl: 'https://github.com/OrleansContrib/OrleansDashboard'
    },

    {
      url: 'assets/images/orleans/rolling upgrade.gif',
      title: 'Rolling upgrade',
      description: '',
      extUrl: 'https://dotnet.github.io/orleans/Documentation/deployment/grain_versioning/grain_versioning.html'
    },

    {
      url: 'assets/images/orleans/server-failure.png',
      title: 'Server failure',
      description: 'The virtual nature of grains allows Orleans to handle server failures mostly transparently to the application logic',
      extUrl: 'https://dotnet.github.io/orleans/Documentation/index.html'
    },
  ];

  constructor(
    config: NgbCarouselConfig
  ) {
    config.interval = 5000;
  }

  ngOnInit() {
  }

}
